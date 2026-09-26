import { useState} from "react";
import {useAuth} from "../context/AuthContext.tsx";
import "./AuthForm.css";

export function RegisterForm({onSwitchToLogin}: {onSwitchToRegister: ()=>void}) {
    const {register} = useAuth();
    const [username, setUsername] = useState("");
    const [password, setPassword] = useState("");
    const [error, setError] = useState("");
    const [submitting, setSubmitting] = useState(false);

    async function handleSubmit(e: React.FormEvent<HTMLFormElement>) {
        e.preventDefault();
        setSubmitting(true);
        setError("");
        try {
            await register(username, password);
        }
        catch (err){
            setError(err instanceof Error ? err.message : "something went wrong");
        }
        finally {
            setSubmitting(false);
        }
    }

    return (
        <form className="auth-form" onSubmit={handleSubmit}>
            <h2>Register</h2>
            <label htmlFor="username">Username</label>
            <input id= "username" type="text" value={username} onChange={(e) => setUsername(e.target.value)}/>
            <label htmlFor="password">Password</label>
            <input id= "password" type="password" value={password} onChange={(e)=>setPassword(e.target.value)}/>
            {error && <p className="error">{error}</p>}
            <button type="submit" disabled={submitting}>Login</button>
            <button type="button" className="switch-link" onClick={onSwitchToLogin}>
                Already have an account? Login
            </button>
        </form>
    );
}