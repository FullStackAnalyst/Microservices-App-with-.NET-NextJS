import { AiOutlineCar } from "react-icons/ai";

const Navbar = () => {
	return (
		<header className="sticky top-0 z-50 flex items-center justify-between bg-white p-5 text-gray-800 shadow-md">
			<div className="flex items-center gap-2 text-3xl font-semibold text-red-500">
				<AiOutlineCar size={35} />
				<h1>Cars</h1>
			</div>
			<div>Search</div>
			<div>Login</div>
		</header>
	);
};

export default Navbar;
