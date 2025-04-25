import React, { useEffect, useRef, useMemo } from 'react';
import * as d3 from 'd3';
import { ChartBalance } from "../../libs/definitions.ts";
import { useAppStore } from "../../libs/stores/app-store.ts";
import apisWrapper from "../../libs/apis-wrapper.ts";

type LinePlotProps = {
    data: ChartBalance[];
    width?: number;
    height?: number;
    marginTop?: number;
    marginRight?: number;
    marginBottom?: number;
    marginLeft?: number;
};

const SimpleLineChart: React.FC<LinePlotProps> = ({
                                                data,
                                                width = 1200,
                                                height = 400,
                                                marginTop = 20,
                                                marginRight = 20,
                                                marginBottom = 30,
                                                marginLeft = 80,
                                            }) => {
    const svgRef = useRef<SVGSVGElement>(null);

    const dimensions = useMemo(() => ({
        innerWidth: width - marginLeft - marginRight,
        innerHeight: height - marginTop - marginBottom,
    }), [width, height, marginLeft, marginRight, marginTop, marginBottom]);

    const parsedData = useMemo(() => {
        const parseTime = d3.timeParse('%Y-%m-%dT%H:%M:%S');
        const result = data.map(d => {
            const parsedDate = parseTime(d.dateTime);
            if (!parsedDate) {
                console.error(`Failed to parse date: ${d.dateTime}`); // دیباگ
                return null;
            }
            return { date: parsedDate, balance: d.balance };
        }).filter(Boolean) as { date: Date; balance: number }[];
        return result;
    }, [data]);

    useEffect(() => {
        if (!svgRef.current || !parsedData.length) return;

        const { innerWidth, innerHeight } = dimensions;

        // پاک کردن محتوای قبلی و ساخت SVG
        const svg = d3.select(svgRef.current);
        svg.selectAll('*').remove(); // پاک کردن محتوای قبلی
        svg.attr('viewBox', `0 0 ${width} ${height}`)
            .attr('width', width)
            .attr('height', height)
            .attr('style', 'max-width: 100%; height: auto; font: 10px JetBrainsMono-Regular;');

        const g = svg.append('g')
            .attr('transform', `translate(${marginLeft},${marginTop})`);

        // دیباگ داده‌ها
        console.log('Parsed Data:', parsedData);

        // مقیاس‌بندی‌ها
        const xScale = d3.scaleTime()
            .domain(d3.extent(parsedData, d => d.date) as [Date, Date])
            .range([0, innerWidth]);

        const yScale = d3.scaleLinear()
            .domain([0, d3.max(parsedData, d => d.balance) as number])
            .nice()
            .range([innerHeight, 0]);

        // خط ساز
        const line = d3.line<{ date: Date; balance: number }>()
            .x(d => xScale(d.date))
            .y(d => yScale(d.balance))
            .curve(d3.curveMonotoneX);

        // محورها و خط
        g.append('g')
            .attr('transform', `translate(0,${innerHeight})`)
            .call(d3.axisBottom(xScale).ticks(Math.max(5, Math.floor(innerWidth / 100))).tickSizeOuter(0));

        g.append('g')
            .call(d3.axisLeft(yScale).ticks(5));

        g.append('path')
            .datum(parsedData)
            .attr('fill', 'none')
            .attr('stroke', '#71717a')
            .attr('stroke-width', 1.3)
            .attr('d', line);

        // استایل‌دهی یکجا
        g.selectAll('.domain, line')
            .attr('stroke', '#71717a');
        g.selectAll('text')
            .attr('stroke', '#71717a')
            .style('font-family', 'JetBrainsMono-Regular');

        if (parsedData.length > 1000) {
            g.attr('shape-rendering', 'crispEdges');
        }
    }, [parsedData, dimensions]); // فقط به parsedData و dimensions وابسته باشه

    return <svg ref={svgRef} />;
};

const SimpleLinePlot: React.FC = () => {
    const [loading, setLoading] = React.useState(true);
    const [data, setData] = React.useState<ChartBalance[]>([]);
    const { selectedPlan } = useAppStore();
    const planId = selectedPlan?.id;

    useEffect(() => {
        const controller = new AbortController();
        if (!planId) {
            setLoading(false); // اگه planId نباشه، لودینگ رو خاموش کن
            return;
        }
        setLoading(true);
        apisWrapper.TradeAnalyseWrapper.getChartBalance(planId, { signal: controller.signal })
            .then((r) => {
                console.log('Received Data:', r); // دیباگ داده‌های دریافتی
                setData(r);
                setLoading(false);
            })
            .catch((err) => {
                if (err.name !== 'AbortError') {
                    // console.error('API Error:', err);
                }
                setLoading(false); // حتی توی خطا هم لودینگ رو خاموش کن
            });

        return () => controller.abort();
    }, [planId]);

    return (
        <div className="bg-zinc-900 m-5 rounded-sm" dir="ltr">
            {loading && <div className="loading">Loading...</div>}
            {!loading && data.length > 0 ? (
                <SimpleLineChart data={data} />
            ) : (
                !loading && <div>No data available</div>
            )}
        </div>
    );
};

export default SimpleLinePlot;