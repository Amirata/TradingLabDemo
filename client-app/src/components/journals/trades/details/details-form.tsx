import {use} from "react";
import {BackBtn} from "../../../ui/my-buttons.tsx";
import {PositionType, Symbols, Trade} from "../../../../libs/definitions.ts";
import {formatDateTime, getEnumName} from "../../../../libs/utilities.ts";

type props = {
    dataPromise: Promise<Trade>;
};

export default function DetailsForm({dataPromise}: props) {

    const data = use(dataPromise);

    return (
        <>
            <div className="flex items-center justify-between p-5">
                <h1 className="text-white/80 font-iran-sans font-bold text-lg">جزئیات ترید پلن {data.tradingPlanName}</h1>
                <BackBtn path="/"/>
            </div>
            <div className="p-5">

                <div className="flex h-screen flex-col gap-4 rounded-sm">
                    <div
                        className="p-5 rounded-sm bg-zinc-700 scrollbar-thumb-rounded-full scrollbar-track-rounded-full scrollbar-thin scrollbar-thumb-zinc-600 scrollbar-track-zinc-500 overflow-y-auto">
                        <div
                            className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-2 font-iran-sans-fa-num leading-8 text-justify text-white/80 font-thin text-sm">
                            <div className="flex items-center bg-zinc-700 p-2 rounded-sm">
                                <span className="pl-2 font-iran-sans-fa-num">نماد:</span>
                                <span className="font-jbm-regular">{getEnumName(Symbols, Number(data.symbol))}</span>
                            </div>
                            <div className="flex items-center bg-zinc-700 p-2 rounded-sm">
                                <span className="pl-2 font-iran-sans-fa-num">جهت:</span>
                                <span
                                    className="font-jbm-regular">{getEnumName(PositionType, Number(data.positionType))}</span>
                            </div>
                            <div className="flex items-center bg-zinc-700 p-2 rounded-sm">
                                <span className="pl-2 font-iran-sans-fa-num">حجم:</span>
                                <span className="font-jbm-regular" dir="ltr">{data.volume}</span>
                            </div>
                            <div className="flex items-center bg-zinc-700 p-2 rounded-sm">
                                <span className="pl-2 font-iran-sans-fa-num">قیمت ورود:</span>
                                <span className="font-jbm-regular" dir="ltr">{data.entryPrice}</span>
                            </div>
                            <div className="flex items-center bg-zinc-700 p-2 rounded-sm">
                                <span className="pl-2 font-iran-sans-fa-num">قیمت خروج:</span>
                                <span className="font-jbm-regular" dir="ltr">{data.closePrice}</span>
                            </div>
                            <div className="flex items-center bg-zinc-700 p-2 rounded-sm">
                                <span className="pl-2 font-iran-sans-fa-num">قیمت استاپ:</span>
                                <span className="font-jbm-regular" dir="ltr">{data.stopLossPrice}</span>
                            </div>
                            <div className="flex items-center bg-zinc-700 p-2 rounded-sm">
                                <span className="pl-2 font-iran-sans-fa-num">زمان ورود:</span>
                                <span className="font-jbm-regular" dir="ltr">{formatDateTime(new Date(data.entryDateTime))}</span>
                            </div>
                            <div className="flex items-center bg-zinc-700 p-2 rounded-sm">
                                <span className="pl-2 font-iran-sans-fa-num">زمان خروج:</span>
                                <span className="font-jbm-regular" dir="ltr">{formatDateTime(new Date(data.closeDateTime))}</span>
                            </div>
                            <div className="flex items-center bg-zinc-700 p-2 rounded-sm">
                                <span className="pl-2 font-iran-sans-fa-num">کمیسیون:</span>
                                <span className="font-jbm-regular" dir="ltr">{data.commission}</span>
                            </div>
                            <div className="flex items-center bg-zinc-700 p-2 rounded-sm">
                                <span className="pl-2 font-iran-sans-fa-num">سواپ:</span>
                                <span className="font-jbm-regular" dir="ltr">{data.swap}</span>
                            </div>
                            <div className="flex items-center bg-zinc-700 p-2 rounded-sm">
                                <span className="pl-2 font-iran-sans-fa-num">پیپ:</span>
                                <span className="font-jbm-regular" dir="ltr">{data.pips}</span>
                            </div>
                            <div className="flex items-center bg-zinc-700 p-2 rounded-sm">
                                <span className="pl-2 font-iran-sans-fa-num">سود:</span>
                                <span className="font-jbm-regular" dir="ltr">{data.netProfit}</span>
                            </div>
                            <div className="flex items-center bg-zinc-700 p-2 rounded-sm">
                                <span className="pl-2 font-iran-sans-fa-num">سود خالص:</span>
                                <span className="font-jbm-regular" dir="ltr">{data.grossProfit}</span>
                            </div>
                            <div className="flex items-center bg-zinc-700 p-2 rounded-sm">
                                <span className="pl-2 font-iran-sans-fa-num">بالانس:</span>
                                <span className="font-jbm-regular" dir="ltr">{data.balance}</span>
                            </div>
                        </div>
                    </div>

                </div>
            </div>
        </>
    );
}
