import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { unstable_HistoryRouter as HistoryRouter } from "react-router-dom";

import './styles/global.css'
import App from './App.tsx'
import "./assets/iransans/iransans.css";
import "./assets/jetbrains-mono/jetbrains-mono.css";
import { history } from "./libs/history.ts";
import Toast from "./components/toastify/toast.tsx";

createRoot(document.getElementById('root')!).render(
  <StrictMode>
      <Toast />
      <HistoryRouter history={history as any}>
          <App />
      </HistoryRouter>
  </StrictMode>,
)
