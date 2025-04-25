// YearListBox.tsx
import {useEffect, useState, useCallback} from "react";
import apisWrapper from "../../libs/apis-wrapper.ts";
import {useAppStore} from "../../libs/stores/app-store.ts";

export const YearListBox = () => {
    const {setSelectedYear, selectedYear, selectedPlan} = useAppStore();
    const [years, setYears] = useState<number[]>([]);
    const {tradeDeleted} = useAppStore();
    const fetchYears = useCallback(async () => {
        if (!selectedPlan?.id) return;

        const response = await apisWrapper.TradeAnalyseWrapper.getGetTradeYears(selectedPlan.id);
        setYears(response);
        if (response.length > 0) {
            setSelectedYear(selectedYear ?? response[0]); // اگر selectedYear null باشه، اولین سال رو ست کن
        }

    }, [selectedPlan?.id]);

    useEffect(() => {
        fetchYears();
    }, [fetchYears, tradeDeleted]);

    return (
        <div className="w-full p-2 rounded-sm">
            <ul className="space-y-2">
                {years.map((year) => (
                    <li
                        key={year}
                        onClick={() => setSelectedYear(year)}
                        className={`cursor-pointer font-jbm-regular text-xs p-1 rounded-sm flex items-center justify-between transition-colors
              ${selectedYear === year
                            ? "bg-zinc-800 text-white/80"
                            : "hover:bg-zinc-600 text-white/80"
                        }`}
                    >
                        <span>{year}</span>
                    </li>
                ))}
            </ul>
        </div>
    );
};
