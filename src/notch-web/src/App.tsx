import {useEffect} from "react";
import {TaskList} from "./components/TaskList";
import {AuthPage} from "./components/AuthPage.tsx";
import {useAuth} from "./context/AuthContext.tsx";

function App()
{
    const {accessToken} = useAuth();
    if(!accessToken)
    {
        return <AuthPage/>;
    }
  return (
      <div className="App">
        <TaskList />
      </div>
  );
}

export default App;