import {useEffect, useState} from "react";
import type {TaskItem} from "../types.ts";

const API_BASE = "http://localhost:5105";

const HARDCODED_TOKEN = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJiM2E4YTBjZC1lOTBlLTQ5MWItYmJhMS04MmQ3YzYzMDJkMmYiLCJ1bmlxdWVfbmFtZSI6ImFobWVkIiwianRpIjoiYmE2NDIwMzEtMTQwZS00MDRjLWI2MTAtMDY0MmE4YmVjYjE5IiwiZXhwIjoxNzkwMTkwNTA5LCJpc3MiOiJOb3RjaCIsImF1ZCI6Ik5vdGNoIn0.TvkHrrv81XaUB3JgwQdwLTViEtWzloF8gtf3OPff6wc";
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