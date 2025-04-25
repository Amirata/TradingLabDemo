import { Suspense } from "react";
import { useParams } from "react-router-dom";

import CreateForm from "./create-form.tsx";
import apisWrapper from "../../../../libs/apis-wrapper.ts";
import {Loader} from "../../../ui/loading/page-loaders.tsx";

export default function CreateTrade() {
  const { id } = useParams<{ id: string }>();
  const dataPromise = apisWrapper.PlanWrapper.getById(id as string);

  return (
    <Suspense fallback={<Loader />}>
      <CreateForm dataPromise={dataPromise} />
    </Suspense>
  );
}
