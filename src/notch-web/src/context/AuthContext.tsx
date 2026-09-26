import {createContext, useContext, useState, type ReactNode} from "react";
import type {TokenResponse} from "../types.ts";
import {API_BASE} from "../config.ts";

interface AuthState {
    accessToken: string | null;
    refreshToken: string | null;
}

function loadInitialState(): AuthState {
    return {
        accessToken: localStorage.getItem("accessToken"),
        refreshToken: localStorage.getItem("refreshToken"),
    };
}

interface AuthContextValue extends AuthState {
    login: (username: string, password: string) => Promise<void>;
    register: (username: string, password: string) => Promise<void>;
    logout: () => void;
}

const AuthContext = createContext<AuthContextValue | null>(null);

export function AuthProvider({ children} : {children: ReactNode}) {
    const [state, setState] = useState<AuthState>(loadInitialState);
    
    // login, register, logout here
    async function login(username: string, password: string)
    {
        const res = await fetch(`${API_BASE}/api/auth/login`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ username, password }),
        });
        if (!res.ok)
        {
            throw new Error("Invalid username or password");
        }
        const data: TokenResponse = await res.json();

        localStorage.setItem("accessToken", data.accessToken);
        localStorage.setItem("refreshToken", data.refreshToken);

        setState({
            accessToken: data.accessToken,
            refreshToken: data.refreshToken,
        });
    }

    async function register(username: string, password: string)
    {
        const res = await fetch(`${API_BASE}/api/auth/register`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ username, password }),
        });
        if (!res.ok)
        {
            const errors = await res.json();
            throw new Error(Array.isArray(errors) ? errors.join("\n") : "Registration failed");
        }
        const data: TokenResponse = await res.json();

        localStorage.setItem("accessToken", data.accessToken);
        localStorage.setItem("refreshToken", data.refreshToken);

        setState({
            accessToken: data.accessToken,
            refreshToken: data.refreshToken,
        });
    }

    function logout()
    {
        localStorage.removeItem("accessToken");
        localStorage.removeItem("refreshToken");

        setState({
            accessToken: null,
            refreshToken: null,
        });
    }
    
    return (
        <AuthContext.Provider value={{...state, login, register,  logout}}>
          {children}
      </AuthContext.Provider>
    );
}

export function useAuth() {
    const context = useContext(AuthContext);
    if( context === null){
        throw new Error("useAuth must be used within a AuthProvider");
    }
    return context;
}