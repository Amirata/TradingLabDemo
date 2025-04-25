import DefaultLayout from "./default.tsx";

export default function PageLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <DefaultLayout>
      <div className="bg-zinc-800 rounded-sm">
        {children}
      </div>
    </DefaultLayout>
  );
}
