import { Suspense } from "react";

import CreateForm from "./create-form.tsx";
import {Loader} from "../../ui/loading/page-loaders.tsx";

export default function CreateUser() {

  return (
    <Suspense fallback={<Loader />}>
      <CreateForm />
    </Suspense>
  );
}
