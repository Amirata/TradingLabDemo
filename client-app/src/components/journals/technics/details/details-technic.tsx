import { useParams } from "react-router-dom";
import { Suspense } from "react";

import DetailsForm from "./details-form.tsx";
import apisWrapper from "../../../../libs/apis-wrapper.ts";
import {Loader} from "../../../ui/loading/page-loaders.tsx";

export default function UpdateTechnic() {
    const { id } = useParams<{ id: string }>();

    const dataPromise = apisWrapper.TechnicWrapper.getById(
        id as string
    );

    return (
        <Suspense fallback={<Loader />}>
            <DetailsForm dataPromise={dataPromise} />
        </Suspense>
    );
}
