import {useEffect} from "react";
import {TaskList} from "./components/TaskList";
import {AuthPage} from "./components/AuthPage.tsx";
import {useAuth} from "./context/AuthContext.tsx";
import {Header} from "./components/Header";
import {useTasks} from "./hooks/useTasks.ts";
import {AddTaskForm} from "./components/AddTaskForm";
import "./App.css";

function App()
{
    const {accessToken} = useAuth();
    const {tasks, loading, error, refetch} = useTasks();
    if(!accessToken)
    {
        return <AuthPage/>;
    }
  return (
      <div className="app-container">
          <Header />
          <AddTaskForm tasks={tasks} onChanged={refetch}/>
          <h2 className="tasks-title">Current Tasks</h2>
        <TaskList tasks={ tasks } loading ={loading} error={error} onChanged={refetch} />
      </div>
  );
}

export default App;