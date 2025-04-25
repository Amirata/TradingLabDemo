// CalendarFooter.tsx
import { YearListBox } from "../../ui/my-listboxes.tsx";
import {useAppStore} from "../../../libs/stores/app-store.ts";
import {useEffect} from "react";
import apisWrapper from "../../../libs/apis-wrapper.ts";


export function CalendarFooter() {

    const { calendarData, selectedPlan, setSelectedYear, selectedYear } = useAppStore();
    //const [yearsLoaded, setYearsLoaded] = useState(false);
    const {tradeDeleted} = useAppStore();
    useEffect(() => {
        if (selectedPlan?.id) {
            const checkYears = async () => {
                const years = await apisWrapper.requests.get<number[]>(
                    `trade-analyse/${selectedPlan.id}`
                );
                //setYearsLoaded(true);
                if (years.length > 0 && !selectedYear) {
                    setSelectedYear(years[0]);
                }
            };
            checkYears();
        }
    }, [selectedPlan?.id, selectedYear, setSelectedYear, tradeDeleted]);
    const renderStats = () => {
        let stats:{label:string, value:number}[] = [];
        if (!calendarData) {
            stats = [
                { label: "میانگین ریسک به ریوارد", value: 0 },
                { label: "وین ریت", value: 0 },
                { label: "تعداد کل معاملات", value: 0 },
                { label: "تعداد معاملات موفق", value: 0 },
                { label: "تعداد معاملات ناموفق", value: 0},
                { label: "سود کل", value: 0 },
                { label: "سود خالص کل", value: 0 },
            ];
        }
        else{
         stats = [
            { label: "میانگین ریسک به ریوارد", value: calendarData.riskToRewardMean },
            { label: "وین ریت", value: calendarData.winRate },
            { label: "تعداد کل معاملات", value: calendarData.totalTradeCount },
            { label: "تعداد معاملات موفق", value: calendarData.totalWinTradeCount },
            { label: "تعداد معاملات ناموفق", value: calendarData.totalLossTradeCount },
            { label: "سود کل", value: calendarData.netProfit },
            { label: "سود خالص کل", value: calendarData.grossProfit },
        ];

        }


        return (
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
                {stats.map(({ label, value }) => (
                    <div key={label}>
                        <span>{label}:</span>
                        <span className="pr-2 font-jbm-regular" dir="ltr">
              {value ?? "N/A"}
            </span>
                    </div>
                ))}
            </div>
        );
    };

    return (
        <div className="mx-5">
        <div className="flex bg-zinc-700 rounded-sm p-5 justify-between w-full">
            <div
                className="w-[100px] border-l border-zinc-500 scrollbar-thin scrollbar-thumb-zinc-600 scrollbar-track-zinc-500 overflow-y-auto"
                dir="ltr"
            >
                <YearListBox />
            </div>
            <div className="w-full p-5 text-white/80 font-iran-sans text-sm">
                {renderStats()}
            </div>
        </div>
        </div>
    );
}