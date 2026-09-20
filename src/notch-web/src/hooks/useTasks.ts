import {useEffect, useState} from "react";
import type {TaskItem} from "../types.ts";

const API_BASE = "http://localhost:5105";

const HARDCODED_TOKEN = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJiZjAxNGFjYS00ZTJhLTQ3ZmMtOWY4OS05M2FmOTgzNTgxYTYiLCJ1bmlxdWVfbmFtZSI6ImFsaSIsImp0aSI6IjczMjZhY2U1LTk1YzktNGQwMC1iMTVlLWNiMDdhZWMxNDhkMyIsImV4cCI6MTc4OTkxNjM0MiwiaXNzIjoiTm90Y2giLCJhdWQiOiJOb3RjaCJ9.chMdNN3u0SvlFb4ZfPQgruZNPRJ5hZVxqAxQMYwBQd8";
export function useTasks() {
    const [tasks, setTasks] = useState<TaskItem[]>([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string>("");

    useEffect(() => {
        setLoading(true);
        fetch(`${API_BASE}/api/tasks`, {
            headers: {
                "Content-Type": "application/json",
                "Accept": "application/json",
                Authorization: `Bearer ${HARDCODED_TOKEN}`
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
    }, []);
    return {tasks, loading, error};
}