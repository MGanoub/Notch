import {useEffect} from "react";
import {TaskList} from "./components/TaskList";

const API_BASE = "http://localhost:5105";

function App()
{
  return (
      <div className="App">
        <TaskList />
      </div>
  );
}

export default App;