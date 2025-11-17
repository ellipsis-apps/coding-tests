// Sidebar.tsx
export default function Sidebar() {
  return (
    <aside className="fixed top-0 left-0 h-screen w-64 bg-gray-800 text-white flex flex-col">
      <div className="p-4 text-2xl font-bold border-b border-gray-700">
        React Currency Test
      </div>
      <nav className="flex-1 p-4 space-y-2">
        <a href="/" className="block px-3 py-2 rounded hover:bg-gray-700">
          Home
        </a>
        <a href="#" className="block px-3 py-2 rounded hover:bg-gray-700">
          Purchase
        </a>
      </nav>
    </aside>
  );
}
