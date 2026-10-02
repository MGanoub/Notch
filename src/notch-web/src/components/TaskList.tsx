import type {TaskItem} from "../types.ts";
import {TaskStatus} from "../types.ts";
import "./TaskList.css";

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
           {getTopLevelTasks(tasks).map((task) => (
               <div key={task.id} className="task-card">
                   <div className="task-row">
                       <span className="task-title">{task.title}</span>
                       <span className={`status-badge ${statusClass(task.status)}`}>
                           {statusLabel(task.status)}
                       </span>
                   </div>
                   {getSubTasks(tasks, task.id).length > 0 &&<ul className="subtask-list">
                       {getSubTasks(tasks, task.id).map((subtask => (
                           <li key={subtask.id} className="subtask-row">
                               <span className={`subtask-title ${subtask.status === TaskStatus.Done ? "done" : ""}`}>
                                   {subtask.title}
                               </span>
                               {subtask.status !== TaskStatus.Done && (
                                   <span className={`status-badge ${statusClass(subtask.status)}`}>
                                       {statusLabel(subtask.status)}
                                   </span>
                               )}
                           </li>
                       )))}
                   </ul>}
               </div>
               ))}
       </div>
    );
}