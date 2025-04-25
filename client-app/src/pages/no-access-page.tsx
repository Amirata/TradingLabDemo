import PageLayout from "../layouts/page-layout.tsx";
import ProtectedRoute from "../components/auth/protected-route.tsx";

export default function NoAccessPage() {
    return (
        <ProtectedRoute>
            <PageLayout>
                <div className="flex flex-col items-center justify-center min-h-[685px]">
                    <div className="flex font-iran-sans text-2xl text-white/80">شما امکان دسترسی به این صفحه را
                        ندارید!
                    </div>
                </div>
            </PageLayout>
        </ProtectedRoute>
    );
}
