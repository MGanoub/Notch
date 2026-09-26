import {useAuth} from "../context/AuthContext.tsx";
import "./Header.css";

export function Header() {
    const {logout} = useAuth();
    return(
        <header className="app-header">
            <div className="app-header-inner">
            <span className="app-title">Notch</span>
            <button className="logout-button" onClick={logout}>Log out</button>
            </div>
        </header>
    );
}