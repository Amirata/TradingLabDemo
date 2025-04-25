import {use} from "react";
import {BackBtn} from "../../../ui/my-buttons.tsx";
import {Plan} from "../../../../libs/definitions.ts";
import {Link} from "react-router-dom";
import GetChartNetProfit from "../ui/chart-net-profit.tsx";
import GetGrossAndNetForEachSymbol from "../ui/gross-and-net-for-each-symbol.tsx";
import GetGrossAndNetForEachSymbolForEachDayOfWeek
    from "../ui/get-gross-and-net-for-each-symbol-for-each-day-of-week.tsx";

type props = {
    dataPromise: Promise<Plan>;
};

export default function DetailsForm({dataPromise}: props) {

    const data = use(dataPromise);
    return (
        <>
            <div className="flex items-center justify-between p-5">
                <h1 className="text-white/80 font-iran-sans font-bold text-lg">جزئیات پلن {data.name}</h1>
                <BackBtn path="/plans"/>
            </div>
            <div className="p-5">

                <div className="flex flex-col gap-4 rounded-sm">
                    <div
                        className="p-5 rounded-sm bg-zinc-700 scrollbar-thumb-rounded-full scrollbar-track-rounded-full scrollbar-thin scrollbar-thumb-zinc-600 scrollbar-track-zinc-500 overflow-y-auto">
                        <div
                            className="max-h-60 h-60 font-iran-sans-fa-num leading-8 text-justify text-white/80 font-thin">
                            <div className="flex items-center bg-zinc-700 p-2 rounded-sm">
                                <span className="pl-2 font-iran-sans-fa-num">از ساعت:</span>
                                <span className="font-jbm-regular">{data.fromTime ? data.fromTime : "--"}</span>
                            </div>
                            <div className="flex items-center bg-zinc-700 p-2 rounded-sm">
                                <span className="pl-2 font-iran-sans-fa-num">تا ساعت:</span>
                                <span className="font-jbm-regular">{data.toTime ? data.toTime : "--"}</span>
                            </div>
                            <div className="flex items-center bg-zinc-700 p-2 rounded-sm">
                                <span className="pl-2 font-iran-sans-fa-num">روزهای:</span>
                                <span
                                    className="font-jbm-regular">{data.selectedDays.length > 0 ? data.selectedDays.toString() : "--"}</span>
                            </div>
                            <div className="flex flex-col bg-zinc-700 p-2 rounded-sm">
                                <span className="font-iran-sans-fa-num">تکنیک ها:</span>
                                {
                                    data.technics.map((technic, i) => {

                                        return <Link className="mt-2" key={i} to={`/technics/details/${technic.id}`}>
                                            <span className="font-iran-sans-fa-num">{i + 1}) {technic.name}</span>
                                        </Link>
                                    })
                                }
                            </div>
                        </div>
                    </div>
                    <GetChartNetProfit/>
                    <GetGrossAndNetForEachSymbol/>
                    <GetGrossAndNetForEachSymbolForEachDayOfWeek/>
                </div>
            </div>
        </>
    );
}
