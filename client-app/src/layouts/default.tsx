//import { Navbar } from "../components/navbar";

import {NavLink} from "react-router-dom";
import {House, Lightbulb, Menu, NotebookPen, Users, X} from "lucide-react";
import React, {ReactNode, useState} from "react";
import {UserSettingsDropdown} from "../components/ui/my-drop-downs.tsx";
import {useAuth} from "../hooks/auth/use-auth.ts";

export default function DefaultLayout({
                                          children,
                                      }: {
    children: React.ReactNode;
}) {

    const [isOpen, setIsOpen] = useState(false);
    const {hasRole} = useAuth();
    return (
        // <div className="font-primary relative flex flex-col h-screen font-iran-sans" dir="rtl">
        //     {/*<Navbar />*/}
        //     <main className="container mx-auto flex-grow">
        //         {children}
        //     </main>
        // </div>

        <div className="min-h-screen bg-zinc-900" dir="rtl">


            {/* Mobile Menu Button */}
            <nav className="md:mr-64 fixed top-0 flex items-center inset-x-0 bg-zinc-800 h-16  z-10">
                <div className="basis-1/2">
                    <UserSettingsDropdown/>
                </div>
                <div className="basis-1/2 flex items-center justify-end">
                    <button
                        className="md:hidden ml-5 p-2 rounded-lg bg-zinc-900 text-white self-end"
                        onClick={() => setIsOpen(!isOpen)}
                        aria-label="Toggle sidebar"
                    >
                        {isOpen ? <X/> : <Menu/>}
                    </button>
                </div>

            </nav>
            {/* Sidebar */}
            <aside
                className={`fixed bg-zinc-800 inset-y-0 right-0 w-64  transform ${
                    isOpen ? 'translate-x-0' : 'translate-x-full'
                } md:translate-x-0 transition-transform duration-300 ease-in-out z-40`}
            >
                <div className="flex flex-col h-full">
                    {/* Logo */}
                    <div className="flex items-center justify-center h-16 ">
                        <h1 className="text-xl font-bold text-white/80 font-jbm-regular">TradingLab</h1>
                    </div>

                    {/* Navigation */}
                    <nav className="flex-1 overflow-y-auto">
                        <ul className="p-4 space-y-2 font-iran-sans">
                            <NavItem to="/" icon={<House/>} label="صفحه اصلی"/>
                            {
                                hasRole("Admin") &&
                                <NavItem to="/users" icon={<Users/>} label="کاربران"/>
                            }
                            <NavItem to="/plans" icon={<NotebookPen/>} label="پلن ها"/>
                            <NavItem to="/technics" icon={<Lightbulb/>} label="تکنیک ها"/>
                        </ul>
                    </nav>

                    {/* Footer */}
                    <div className="p-4 border-t border-t-zinc-600 font-jbm-regular text-left" dir="ltr">
                        <p className="text-sm text-zinc-500">© {new Date().getFullYear()} TradingLab</p>
                    </div>
                </div>
            </aside>

            {/* Main Content */}
            <main className="md:mr-64 mt-16 p-6 min-h-screen">

                <div className="max-w-7xl mx-auto">{children}</div>
            </main>
        </div>
    )
        ;
};

const NavItem = ({to, icon, label}: { to: string, icon: ReactNode, label: string }) => {
    return (
        <li>
            <NavLink
                to={to}
                className={({isActive}) =>
                    `flex items-center p-2 rounded-sm transition-colors ${
                        isActive
                            ? 'bg-zinc-400 text-zinc-700'
                            : 'hover:bg-zinc-500 text-zinc-600'
                    }`
                }
            >
                <span className="ml-2 size-6">{icon}</span>
                <span className="font-medium text-sm">{label}</span>
            </NavLink>
        </li>
    );
}
