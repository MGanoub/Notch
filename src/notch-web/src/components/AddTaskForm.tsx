import {useState} from "react";
import {useAuth} from "../context/AuthContext.tsx";
import {API_BASE} from "../config";
import type {TaskItem} from "../types";
import "./Addtaskform.css";

interface AddTaskFormProps {
    tasks: TaskItem [];
    onChanged: ()=> void;
}

export function AddTaskForm({tasks, onChanged}: AddTaskFormProps) {
    const {accessToken} = useAuth();
    const [title, setTitle] = useState<string>("");
    const [parentTaskId, setParentTaskId] = useState<string>("");
    const [submitting, setSubmitting] = useState(false);
    const [error, setError] = useState<string>("");
    
    async function handleSubmit(e: React.FormEvent<HTMLFormElement>) {
        e.preventDefault();
        setSubmitting(true);
        setError("");
        try {
            const res = await fetch(`${API_BASE}/api/tasks`, {
               headers: new Headers({ Authorization: `Bearer ${accessToken}`,
                   "Content-Type": "application/json"}),
               method: "POST",
               body: JSON.stringify({title: title, parentTaskId: parentTaskId || null}), 
            });
            if (!res.ok) {
                throw new Error("Failed to create task" );
            }
            setTitle("");
            setParentTaskId("");
            onChanged();
        } catch (err) {
            setError(err instanceof Error ? err.message : "Something went wrong");
        }finally {
            setSubmitting(false);
        }
        // post task to add
    }
    
    return (
        <form className="add-task-form" onSubmit={handleSubmit}>
            <h2>New task</h2>
            <div className="form-row">
            <input 
                id="addTaskInput" 
                type="Text"
                value={title}
                onChange={e => setTitle(e.target.value)} />
            <select 
                id="taskParent" 
                value={parentTaskId}
                onChange={e => setParentTaskId(e.target.value)}>
                <option value="">No parent</option>
                {
                    tasks
                        .filter((task) => task.parentTaskId === null)
                        .map((task) =>(
                        <option key={task.id} value={task.id}>
                            {task.title}
                        </option>
                    ))};
            </select>
            <button type="submit" disabled={submitting || (title.trim() === "")}>Add</button>
            </div>
            {error && (<p> {error}</p>)}
        </form>
    );
}

