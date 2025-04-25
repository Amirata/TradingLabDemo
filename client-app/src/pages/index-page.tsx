import Main from "../components/journals/main/main.tsx";
import PageLayout from "../layouts/page-layout.tsx";
import ProtectedRoute from "../components/auth/protected-route.tsx";

export default function IndexPage() {
    return (
        <ProtectedRoute>
            <PageLayout>
                <Main/>
            </PageLayout>
        </ProtectedRoute>
    );
}
