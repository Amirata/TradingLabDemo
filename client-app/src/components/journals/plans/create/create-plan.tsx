import { Suspense } from "react";
import CreateForm from "./create-form.tsx";
import apisWrapper from "../../../../libs/apis-wrapper.ts";
import {Loader} from "../../../ui/loading/page-loaders.tsx";

export default function CreatePlan() {

    const dataPromise = apisWrapper.TechnicWrapper.list({
        pageNumber:1,
        pageSize:100000,
        search:"",
        sorts:[]
    });

  return (
    <Suspense fallback={<Loader />}>
      <CreateForm dataPromise={dataPromise} />
    </Suspense>
  );
}
