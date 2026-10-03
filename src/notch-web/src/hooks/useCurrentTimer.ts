import { useEffect, useState } from "react";
import { useAuth } from "../context/AuthContext";
import { API_BASE } from "../config";
import type { TimeEntryDto } from "../types";

export function useCurrentTimer() {
    const { accessToken } = useAuth();
    const [currentEntry, setCurrentEntry] = useState<TimeEntryDto | null>(null);

    async function fetchCurrent() {
        if (!accessToken) return;
        const res = await fetch(`${API_BASE}/api/timeentries/current`, {
            headers: { Authorization: `Bearer ${accessToken}` },
        });
        if (res.ok) {
            setCurrentEntry(await res.json());
        }
    }

    useEffect(() => {
        fetchCurrent();
    }, [accessToken]);

    async function start(taskId: string) {
        const res = await fetch(`${API_BASE}/api/timeentries/start`, {
            method: "POST",
            headers: new Headers({
                Authorization: `Bearer ${accessToken}`,
                "Content-Type": "application/json",
            }),
            body: JSON.stringify({ taskItemId: taskId }),
        });
        if (res.ok) {
            setCurrentEntry(await res.json());
        }
    }

    async function stop() {
        const res = await fetch(`${API_BASE}/api/timeentries/stop`, {
            method: "POST",
            headers: new Headers({ Authorization: `Bearer ${accessToken}` }),
        });
        if (res.ok || res.status === 204) {
            setCurrentEntry(null);
        }
    }

    return { currentEntry, start, stop };
}