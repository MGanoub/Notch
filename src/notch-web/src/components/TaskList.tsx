import {useTasks} from "../hooks/useTasks.ts";
import type {TaskItem} from "../types.ts";

export function TaskList()
{
    const {tasks, loading, error} = useTasks();
    if(loading)
    {
        return <p>Loading...</p>;
    }
    if(error)
    {
        return <p> {error}</p>;
    }
    return (
       <ul>
           {getTopLevelTasks(tasks).map((task) => (
               <li key={task.id}>
                   {task.title}
                   { getSubTasks(tasks, task.id).map((subtask) => {return <p key={subtask.id}>{subtask.title}</p>}) }
               </li>
               ))}
       </ul>
    );
}

function getSubTasks(tasks: TaskItem[], parentId: string) : TaskItem[] {
    return tasks.filter((task) => {return task.parentTaskId === parentId});
}

function getTopLevelTasks(tasks: TaskItem []) : TaskItem[] {
    return tasks.filter((task) => { return task.parentTaskId === null});
}