import { useState } from "react";
import { LoginForm } from "./LoginForm";
import { RegisterForm } from "./RegisterForm";
import "../theme.css";
import "./AuthPage.css"

export function AuthPage(){
    const [showRegister, setShowRegister] = useState(false);
    
    return (
        <div>
            <h2 className="form-title">Notch</h2>
            {showRegister ? (<RegisterForm onSwitchToLogin={()=> setShowRegister(false)} />) : <LoginForm onSwitchToRegister={()=> setShowRegister(true)}/>  }
        </div>
    );
}