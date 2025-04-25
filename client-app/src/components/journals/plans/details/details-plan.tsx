import { useParams } from "react-router-dom";
import { Suspense } from "react";

import apisWrapper from "../../../../libs/apis-wrapper.ts";

import DetailsForm from "./details-form.tsx";
import {Loader} from "../../../ui/loading/page-loaders.tsx";

export default function DetailsPlan() {
  const { id } = useParams<{ id: string }>();

  const dataPromise = apisWrapper.PlanWrapper.getById(id as string);

  return (
    <Suspense fallback={<Loader />}>
      <DetailsForm dataPromise={dataPromise} />
    </Suspense>
  );
}
