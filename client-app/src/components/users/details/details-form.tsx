import {use} from "react";
import {BackBtn} from "../../ui/my-buttons.tsx";
import {User} from "../../../libs/definitions.ts";

type props = {
    dataPromise: Promise<User>;
};

export default function DetailsForm({dataPromise}: props) {

    const data = use(dataPromise);

    return (
        <>
            <div className="flex items-center justify-between p-5">
                <h1 className="text-white/80 font-iran-sans font-bold text-lg">جزئیات کاربر</h1>
                <BackBtn path="/users"/>
            </div>
            <div className="p-5">

                <div className="flex h-screen flex-col gap-4 rounded-sm">
                    <div
                        className="p-5 rounded-sm bg-zinc-700 scrollbar-thumb-rounded-full scrollbar-track-rounded-full scrollbar-thin scrollbar-thumb-zinc-600 scrollbar-track-zinc-500 overflow-y-auto">
                        <div
                            className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-2 font-iran-sans-fa-num leading-8 text-justify text-white/80 font-thin text-sm">
                            <div className="flex items-center bg-zinc-700 p-2 rounded-sm">
                                <span className="pl-2 font-iran-sans-fa-num">نام کاربری:</span>
                                <span className="font-jbm-regular">{data.userName}</span>
                            </div>
                            <div className="flex items-center bg-zinc-700 p-2 rounded-sm">
                                <span className="pl-2 font-iran-sans-fa-num">پست الکترونیکی:</span>
                                <span
                                    className="font-jbm-regular">{data.email}</span>
                            </div>
                        </div>
                    </div>

                </div>
            </div>
        </>
    );
}
