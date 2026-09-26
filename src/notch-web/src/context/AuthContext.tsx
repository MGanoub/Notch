import {createContext, useContext, useState, type ReactNode} from "react";
import type {TokenResponse} from "../types.ts";

interface AuthContext {
    accessToken: string | null;
    refreshToken: string | null;
}

function loadInitialState(): AuthContext {
    return {
        accessToken: localStorage.getItem("accessToken"),
        refreshToken: localStorage.getItem("refreshToken"),
    };
}