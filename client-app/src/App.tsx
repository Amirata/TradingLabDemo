import {Route, Routes} from 'react-router-dom'
import './App.css'
import { AuthProvider } from './contexts/auth-context.tsx'
import LoginPage from "./pages/login-page.tsx";
import UserSettingsPage from "./pages/user-settings-page.tsx";
import TechnicsPage from "./pages/technics-page.tsx";
import PlansPage from "./pages/plans-page.tsx";
import TradesPage from "./pages/trades-page.tsx";
import IndexPage from "./pages/index-page.tsx";
import Technics from "./components/journals/technics/technics.tsx";
import CreateTechnic from "./components/journals/technics/create/create-technic.tsx";
import UpdateTechnic from "./components/journals/technics/update/update-technic.tsx";
import DetailsTechnic from './components/journals/technics/details/details-technic.tsx';
import Plans from "./components/journals/plans/plans.tsx";
import CreatePlan from "./components/journals/plans/create/create-plan.tsx";
import UpdatePlan from "./components/journals/plans/update/update-plan.tsx";
import DetailsPlan from "./components/journals/plans/details/details-plan.tsx";
import DetailsTrade from "./components/journals/trades/details/details-trade.tsx";
import CreateTrade from "./components/journals/trades/create/create-trade.tsx";
import UpdateTrade from "./components/journals/trades/update/update-trade.tsx";
import UsersPage from './pages/users-page.tsx';
import CreateUser from "./components/users/create/create-user.tsx";
import UpdateUser from "./components/users/update/update-user.tsx";
import DetailsUser from "./components/users/details/details-user.tsx";
import Users from "./components/users/users.tsx";
import NoAccessPage from "./pages/no-access-page.tsx";


function App() {

    return (

        <AuthProvider>
            <Routes>
                <Route element={<LoginPage />} path="/login" />

                <Route element={<IndexPage />} path="/" />
                <Route element={<NoAccessPage />} path="/no-access" />
                <Route element={<UserSettingsPage />} path="/userSetting" />
                <Route element={<TechnicsPage />} path="technics">
                    <Route element={<CreateTechnic />} path="create" />
                    <Route element={<UpdateTechnic />} path="update/:id" />
                    <Route element={<DetailsTechnic />} path="details/:id" />
                    <Route index element={<Technics />} />
                </Route>
                <Route element={<PlansPage />} path="plans">
                    <Route element={<CreatePlan />} path="create" />
                    <Route element={<UpdatePlan />} path="update/:id" />
                    <Route element={<DetailsPlan />} path="details/:id" />
                    <Route index element={<Plans />} />
                </Route>
                <Route element={<TradesPage />} path="trades">
                    <Route element={<CreateTrade />} path="create/:id" />
                    <Route element={<UpdateTrade />} path="update/:id" />
                    <Route element={<DetailsTrade />} path="details/:id" />
                </Route>
                <Route element={<UsersPage />} path="users">
                    <Route element={<CreateUser />} path="create" />
                    <Route element={<UpdateUser />} path="update/:id" />
                    <Route element={<DetailsUser />} path="details/:id" />
                    <Route index element={<Users />} />
                </Route>
            </Routes>
        </AuthProvider>
    )
}

export default App
