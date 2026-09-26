import {useEffect, useState} from "react";
import type {TaskItem} from "../types.ts";
import {API_BASE} from "../config.ts";
import {useAuth} from "../context/AuthContext.tsx";

export function useTasks() {
    const {accessToken } = useAuth();
    const [tasks, setTasks] = useState<TaskItem[]>([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string>("");

    useEffect(() => {
        if(!accessToken)
        {
            return;
        }
        setLoading(true);
        fetch(`${API_BASE}/api/tasks`, {
            headers: {
                "Content-Type": "application/json",
                "Accept": "application/json",
                Authorization: `Bearer ${accessToken}`
            }
        })
            .then((res) => {
                if (!res.ok) {
                    throw new Error("Failed to fetch tasks");
                }
                return res.json();
            })
            .then((data) => {
                setTasks(data)
            })
            .catch((err) => setError(err.message))
            .finally(() => setLoading(false));
    }, [accessToken]);
    return {tasks, loading, error};
}