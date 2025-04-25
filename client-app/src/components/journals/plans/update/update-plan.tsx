import { useParams } from "react-router-dom";
import { Suspense } from "react";

import UpdateForm from "./update-form.tsx";
import apisWrapper from "../../../../libs/apis-wrapper.ts";
import {Loader} from "../../../ui/loading/page-loaders.tsx";

export default function UpdatePlan() {
  const { id } = useParams<{ id: string }>();

  const technicsDataPromise = apisWrapper.TechnicWrapper.list({
      pageNumber:1,
      pageSize:100000,
      search:"",
      sorts:[]
  });

  const planDataPromise = apisWrapper.PlanWrapper.getById(id as string);

  return (
    <Suspense fallback={<Loader />}>
      <UpdateForm
        planDataPromise={planDataPromise}
        technicsDataPromise={technicsDataPromise}
      />
    </Suspense>
  );
}
