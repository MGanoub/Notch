import type {TaskItem} from "../types.ts";
import {TaskStatus} from "../types.ts";
import "./TaskList.css";
import { useAuth } from "../context/AuthContext.tsx";
import { API_BASE } from "../config";

interface TaskListProps {
    tasks : TaskItem[];
    loading: boolean;
    error: string;
    onChanged: () => void;
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

export function TaskList( {tasks, loading, error, onChanged} : TaskListProps)
{
    const {accessToken } = useAuth();

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
               return (
               <div key={task.id} className="task-card">
                   <div className="task-row">
                       <span className="task-title">{task.title}</span>
                       <select 
                           className={`status-select ${statusClass(task.status)}`}
                            value={task.status}
                       onChange={(e) => updateStatus(task.id, Number(e.target.value) as TaskStatus)}>
                           <option value={TaskStatus.Todo}>{statusLabel(TaskStatus.Todo)}</option>
                           <option value={TaskStatus.InProgress}>{statusLabel(TaskStatus.InProgress)}</option>
                           <option value={TaskStatus.Done}>{statusLabel(TaskStatus.Done)}</option>
                       </select>
                   </div>
                   {subtasks.length > 0 && <ul className="subtask-list">
                       {subtasks.map((subtask => (
                           <li key={subtask.id} className="subtask-row">
                               <span className={`subtask-title ${subtask.status === TaskStatus.Done ? "done" : ""}`}>
                                   {subtask.title}
                               </span>
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
                       )))}
                   </ul>}
               </div>
               )})}
       </div>
    );
}