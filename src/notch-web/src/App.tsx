import {useEffect} from "react";
import {TaskList} from "./components/TaskList";
import {AuthPage} from "./components/AuthPage.tsx";
import {useAuth} from "./context/AuthContext.tsx";
import {Header} from "./components/Header";
import "./App.css";

function App()
{
    const {accessToken} = useAuth();
    if(!accessToken)
    {
        return <AuthPage/>;
    }
  return (
      <div className="app-container">
          <Header />
        <TaskList />
      </div>
  );
}

export default App;