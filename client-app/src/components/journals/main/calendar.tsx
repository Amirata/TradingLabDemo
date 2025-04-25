// CalendarPart.tsx
import { useEffect, useState, useCallback } from "react";
import { ActivityCalendar } from "../../calendar/ActivityCalendar.tsx";
import apisWrapper from "../../../libs/apis-wrapper.ts";
import { CalendarData, Calendar } from "../../../libs/definitions.ts";
import { useAppStore } from "../../../libs/stores/app-store.ts";
import { ChartLoader } from "../../ui/loading/page-loaders.tsx";

export default function CalendarPart() {
    const { selectedPlan, selectedYear, setCalendarData } = useAppStore();
    const [calendar, setCalendar] = useState<CalendarData | null>(null);
    const [loading, setLoading] = useState(false);
    const {tradeDeleted} = useAppStore();

    const fetchCalendarData = useCallback(
        async (planId: string, year: number, signal: AbortSignal) => {
            try {
                setLoading(true);
                const data = await apisWrapper.TradeAnalyseWrapper.getCalenderData(
                    planId,
                    year,
                    { signal }
                );
                setCalendar(data);
                setCalendarData(data);
            } catch (error:any) {
                if (error.name !== "AbortError") {
                    //console.error("Failed to fetch calendar data:", error);
                    setCalendar(null);
                    setCalendarData(null);
                }
            } finally {
                setLoading(false);
            }
        },
        []
    );

    useEffect(() => {
        const controller = new AbortController();
        if (selectedPlan?.id && selectedYear) {
            fetchCalendarData(selectedPlan.id, selectedYear, controller.signal);
        } else {
            setLoading(false);
            setCalendar(null);
            setCalendarData(null);
        }
        return () => controller.abort();
    }, [selectedPlan?.id, selectedYear, fetchCalendarData, tradeDeleted]);



    const renderBody = useCallback(() => (

            <div
                className="hidden lg:flex lg:flex-col lg:justify-center lg:min-h-[161px] text-white/80 font-jbm-regular"
                dir="ltr"
            >
                {calendar && Array.isArray(calendar.calendar) ? (
                    <ActivityCalendar
                        data={calendar.calendar as Calendar[]}
                        maxLevel={2}
                        showWeekdayLabels
                        theme={{
                            light: ["#220000", "#c4edde", "#00ff00"],
                            dark: ["#dc2626", "#3f3f46", "#65a30d"],
                        }}
                    />
                ) : (
                    <div className="text-white/80 font-iran-sans" dir="rtl">داده‌ای برای نمایش وجود ندارد!</div>
                )}
            </div>

    ), [calendar]);

    return (
        <div className="hidden lg:block bg-zinc-900 p-5 mx-5 min-h-[200px] text-white/80 rounded-sm">
            {loading && <ChartLoader />}
            {!loading && (
                <div className="flex-grow flex flex-col items-center h-full">
                    { selectedYear ? (
                        renderBody()
                    ) : (
                        <div className="flex h-full justify-center items-center font-iran-sans text-white/80" dir="rtl">
                            داده‌ای برای نمایش وجود ندارد!
                        </div>
                    )}
                </div>
            )}
        </div>
    );
}