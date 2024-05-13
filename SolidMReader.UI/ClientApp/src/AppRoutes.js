import { Home } from "./components/Home";
import { Login } from "./components/Login";
import { Upload } from "./components/Upload";

const AppRoutes = [
  {
    index: true,
    element: <Home />
  },
  {
    path: '/upload',
    element: <Upload />
  },
  {
    path: '/login',
    element: <Login />
  }
];

export default AppRoutes;
