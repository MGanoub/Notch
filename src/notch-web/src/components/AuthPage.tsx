import { useState } from "react";
import { LoginForm } from "./LoginForm";
import { RegisterForm } from "./RegisterForm";

export function AuthPage(){
    const [showRegister, setShowRegister] = useState(false);
    
    if(showRegister){
        return <RegisterForm onSwitchToLogin={()=> setShowRegister(false)}/>
    }
    return <LoginForm onSwitchToRegister={()=> setShowRegister(true)}/>
}