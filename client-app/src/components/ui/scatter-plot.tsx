import * as d3 from "d3";
import { useRef, useEffect, useState } from "react";

type LinePlotProps = {
    data: ExperienceData;
    width?: number;
    height?: number;
    marginTop?: number;
    marginRight?: number;
    marginBottom?: number;
    marginLeft?: number;
};

type FrameworkId = "react" | "vuejs" | "angular" | "preact" | "ember" | "svelte" | "alpinejs" | "litelement" | "stimulus" | "solid";

type Year = 2016 | 2017 | 2018 | 2019 | 2020 | 2021;

type ExperienceMetric = {
    year: Year;
    rank: number;
    percentage_question: number;
};

type FrameworkExperience = {
    id: FrameworkId;
    name: string;
    user_count: number;
    retention_percentage: number;
    usage: ExperienceMetric[];
    awareness: ExperienceMetric[];
    interest: ExperienceMetric[];
    satisfaction: ExperienceMetric[];
};

type ExperienceData = {
    ids: FrameworkId[];
    years: Year[];
    experience: FrameworkExperience[];
};

export default function ScatterPlot({
                                     data,
                                     width = 800,
                                     height = 300,
                                     marginTop = 20,
                                     marginRight = 20,
                                     marginBottom = 30,
                                     marginLeft = 20,
                                 }: LinePlotProps) {
    const colorScale = d3.scaleOrdinal()
        .domain(data.ids)
        .range(d3.schemeTableau10);

    const innerWidth = width - marginLeft - marginRight;
    const innerHeight = height - marginTop - marginBottom;

    const scatterplotRef = useRef<SVGGElement>(null);
    useEffect(() => {
        const scatterplotContainer = d3.select(scatterplotRef.current);
        scatterplotContainer.selectAll("*").remove(); // Clear before re-rendering

        // Declare scales
        const xScale = d3.scaleLinear()
            .domain([0, d3.max(data.experience, d => d.user_count) || 0]) // Handle undefined case
            .range([0, innerWidth])
            .nice();

        const yScale = d3.scaleLinear()
            .domain([0, 100])
            .range([innerHeight, 0]);

        // Append axes
        const bottomAxis = d3.axisBottom(xScale)
            .ticks(width/80)
            .tickSizeOuter(0)
            .tickFormat(d3.format("d"));

        scatterplotContainer.append("g")
            .attr("class", "axis")
            .attr("transform", `translate(0, ${innerHeight})`)
            .call(bottomAxis);

        const leftAxis = d3.axisLeft(yScale).ticks(5);


        scatterplotContainer.append("g")
            .attr("class", "axis")
            .call(leftAxis);

        // Append circles
        scatterplotContainer.selectAll(".scatterplot-circle")
            .data(data.experience)
            .join("circle")
            .attr("class", "scatterplot-circle")
            .attr("cx", d => xScale(d.user_count))
            .attr("cy", d => yScale(d.retention_percentage))
            .attr("r", 6)
            .attr("fill", d => colorScale(d.id) as string);

    }, [data, innerWidth, innerHeight]);


    return (
        <div>
            <h2>Retention vs Usage</h2>
            <svg viewBox={`0 0 ${width} ${height}`}>
                <g transform={`translate(${marginLeft}, ${marginTop})`}>
                    <g ref={scatterplotRef}></g>
                </g>
            </svg>
        </div>
)
}

export function MainScatterPlot() {
    const [loading, setLoading] = useState(true);
    const [data, setData] = useState<ExperienceData>();

    useEffect(() => {
        const dataURL = "https://d3js-in-action-third-edition.github.io/hosted-data/apis/front_end_frameworks.json";
        const controller = new AbortController();

        d3.json<ExperienceData>(dataURL, { signal: controller.signal }).then(data => {
            console.log("data", data);
            setData(data);
            setLoading(false);
        }).catch(err => {
            if (err.name !== "AbortError") {
                console.error("Failed to fetch data:", err);
            }
        });

        return () => controller.abort(); // Cancel fetch on unmount
    }, []);

    return (
        <div className="bg-zinc-900 m-5 rounded-sm">
            {loading && <div className="loading">Loading...</div>}
            {!loading && data && <ScatterPlot data={data as ExperienceData} />}
        </div>
    );
}
