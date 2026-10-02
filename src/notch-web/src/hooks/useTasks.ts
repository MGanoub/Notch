import {useEffect, useState} from "react";
import type {TaskItem} from "../types.ts";
import {API_BASE} from "../config.ts";
import {useAuth} from "../context/AuthContext.tsx";

export function useTasks() {
    const {accessToken } = useAuth();
    const [tasks, setTasks] = useState<TaskItem[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string>("");
    
    async function fetchTasks() {
        if(!accessToken) return;
        setLoading(true);
        try {
            const res = await fetch(`${API_BASE}/api/tasks`, {
                headers: { Authorization: `Bearer ${accessToken}` },
            });
            if(!res.ok) throw new Error("failed to fetch tasks");
            setTasks(await res.json());
        }
        catch (err){
            setError( err instanceof Error ? err.message : "something went wrong" );
        } finally {
            setLoading(false);
        }
    }
    useEffect(() => {
        fetchTasks();
    }, [accessToken]);
    return {tasks, loading, error, refetch: fetchTasks};
}