import Sidebar from "./components/sidebar"

function App() {
  return (
    <div className="flex">
      <Sidebar />
      <main className="ml-64 flex-1 p-6">
        <h1 className="text-3xl font-bold">Welcome!</h1>
        <p>This is the main content area.</p>
      </main>
    </div>
  );
}

export default App;
