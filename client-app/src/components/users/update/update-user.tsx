import { useParams } from "react-router-dom";
import { Suspense } from "react";

import UpdateForm from "./update-form.tsx";
import apisWrapper from "../../../libs/apis-wrapper.ts";
import {Loader} from "../../ui/loading/page-loaders.tsx";

export default function UpdateUser() {
  const { id } = useParams<{ id: string }>();

  const dataPromise = apisWrapper.UserWrapper.getById(id as string);

  return (
    <Suspense fallback={<Loader />}>
      <UpdateForm dataPromise={dataPromise} />
    </Suspense>
  );
}
