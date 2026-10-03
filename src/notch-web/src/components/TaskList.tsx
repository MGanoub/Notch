import type {TaskItem} from "../types.ts";
import type {TimeEntryDto} from "../types.ts";
import {TaskStatus} from "../types.ts";
import "./TaskList.css";
import { useAuth } from "../context/AuthContext.tsx";
import { API_BASE } from "../config";
import {useEffect, useState} from "react";
import { IconPencil, IconCheck, IconX } from "@tabler/icons-react";


interface TaskListProps {
    tasks : TaskItem[];
    loading: boolean;
    error: string;
    onChanged: () => void;
    currentTask: TimeEntryDto | null;
    startTask: (task: TaskItem) => void;
    stopTask: (task: TaskItem) => void;
}

function getSubTasks(tasks: TaskItem[], parentId: string) : TaskItem[] {
    return tasks.filter((task) => {return task.parentTaskId === parentId});
}

function getTopLevelTasks(tasks: TaskItem []) : TaskItem[] {
    return tasks.filter((task) => { return task.parentTaskId === null});
}

function statusLabel(status: TaskStatus) : string{
    switch(status){
        case TaskStatus.Todo: return "Todo";
        case TaskStatus.InProgress: return "In progress";
        case TaskStatus.Done: return "Done";
    }
}

function statusClass(status: TaskStatus): string {
    switch (status) {
        case TaskStatus.Todo: return "todo";
        case TaskStatus.InProgress: return "in-progress";
        case TaskStatus.Done: return "done";
    }
}

export function TaskList( {tasks, loading, error, onChanged, currentTask, startTask, stopTask} : TaskListProps)
{
    const {accessToken } = useAuth();
    const [editingTaskId, setEditingTaskId] = useState<string | null>(null);
    const [editedTitle, setEditedTitle] = useState<string>("");

    useEffect(() => {
        onChanged();
    }, []);
    
    function startEditing(task : TaskItem) : void {
        setEditingTaskId(task.id);
        setEditedTitle(task.title);
    }
    
    function cancelEditing() : void {
        setEditingTaskId(null);
        setEditedTitle("");
    }
    
    async function saveEdit(taskId: string) {
        const trimmed = editedTitle.trim();
        if(trimmed === "")
        {
            return;
        }
        try {
            const res = await fetch(`${API_BASE}/api/tasks/${taskId}/title`, {
                method: "PATCH",
                headers: new Headers({
                    Authorization: `Bearer ${accessToken}`,
                    "Content-Type": "application/json",
                }),
                body: JSON.stringify({title: trimmed}),
            });
            if(!res.ok)
            {
                throw new Error("failed to update title");
            }
            onChanged();
            setEditingTaskId(null);
        } catch(err)
        {
            console.log(err instanceof Error ? err.message : "Something went wrong");
        }
    }

    async function updateStatus(taskId: string, newStatus:TaskStatus)
    {
        try{
            const res = await fetch(`${API_BASE}/api/tasks/${taskId}/status`, {
                method: "PATCH",
                headers: new Headers({
                    Authorization: `Bearer ${accessToken}`,
                    "Content-Type": "application/json"}),
                body: JSON.stringify({status: newStatus}),
            });
            if(!res.ok)
            {
                throw new Error("Failed to update task");
            }
            onChanged();
        }
        catch (err) {
            console.log(err instanceof Error ? err.message : "something went wrong");
        }
    }

    async function handleOnClicked(task: TaskItem)
    {
        const isTaskRunning = (currentTask !== null && task.id === currentTask.taskItemId);
        if(isTaskRunning)
        {
            await stopTask();
        }
        else
        {
            await startTask(task.id);
        }
        onChanged();
    }
    
    if(loading)
    {
        return <p>Loading...</p>;
    }
    if(error)
    {
        return <p> {error}</p>;
    }
    console.log(tasks);
    return (
       <div className="task-list">
           {getTopLevelTasks(tasks).map((task) => {
               const subtasks = getSubTasks(tasks, task.id);
               const isCurrentTask = (currentTask !== null && task.id === currentTask.taskItemId);
               const isEditing = editingTaskId === task.id;
               return (
               <div key={task.id} className="task-card">
                   <div className="task-row">
                       {isEditing ? (
                           <div className="title-edit">
                               <input
                               autoFocus
                               value={editedTitle}
                               onChange={(e) => setEditedTitle(e.target.value)}
                               onKeyDown={(e) => {
                                   if(e.key === "Enter") saveEdit(task.id);
                                   if(e.key === "Escape") cancelEditing();
                               }}
                               />
                               <IconCheck size={16} className="icon-btn" onClick={() => saveEdit(task.id)} />
                               <IconX size={16} className="icon-btn" onClick={cancelEditing} />
                           </div>
                       ) : (
                           <div className="title-display">
                               <span className="task-title">{task.title}</span>
                               <IconPencil size={12} className="icon-btn" onClick={() => startEditing(task)} />
                           </div>
                       )}
                       <select 
                           className={`status-select ${statusClass(task.status)}`}
                            value={task.status}
                       onChange={(e) => updateStatus(task.id, Number(e.target.value) as TaskStatus)}>
                           <option value={TaskStatus.Todo}>{statusLabel(TaskStatus.Todo)}</option>
                           <option value={TaskStatus.InProgress}>{statusLabel(TaskStatus.InProgress)}</option>
                           <option value={TaskStatus.Done}>{statusLabel(TaskStatus.Done)}</option>
                       </select>
                   </div>
                   {subtasks.length > 0 && (
                       <ul className="subtask-list">
                           {subtasks.map((subtask) => {
                               const isSubtaskEditing = editingTaskId === subtask.id;
                               return (
                                   <li key={subtask.id} className="subtask-row">
                                       {isSubtaskEditing ? (
                                           <div className="title-edit">
                                               <input
                                                   autoFocus
                                                   value={editedTitle}
                                                   onChange={(e) => setEditedTitle(e.target.value)}
                                                   onKeyDown={(e) => {
                                                       if (e.key === "Enter") saveEdit(subtask.id);
                                                       if (e.key === "Escape") cancelEditing();
                                                   }}
                                               />
                                               <IconCheck size={16} className="icon-btn" onClick={() => saveEdit(subtask.id)} />
                                               <IconX size={16} className="icon-btn" onClick={cancelEditing} />
                                           </div>
                                       ) : (
                                           <div className="title-display">
                                                    <span className={`subtask-title ${subtask.status === TaskStatus.Done ? "done" : ""}`}>
                                                        {subtask.title}
                                                    </span>
                                               <IconPencil size={12} className="icon-btn" onClick={() => startEditing(subtask)} />
                                           </div>
                                       )}
                                       <select
                                           className={`status-select ${statusClass(subtask.status)}`}
                                           value={subtask.status}
                                           onChange={(e) => updateStatus(subtask.id, Number(e.target.value) as TaskStatus)}
                                       >
                                           <option value={TaskStatus.Todo}>{statusLabel(TaskStatus.Todo)}</option>
                                           <option value={TaskStatus.InProgress}>{statusLabel(TaskStatus.InProgress)}</option>
                                           <option value={TaskStatus.Done}>{statusLabel(TaskStatus.Done)}</option>
                                       </select>
                                   </li>
                               );
                           })}
                       </ul>
                   )}
               </div>
               );
           })}
       </div>
    );
}