async function getData() {
	const res = await fetch("https://localhost:7001/auction");

	if (!res.ok) {
		throw new Error("Failed to fetch listings");
	}

	const data = await res.json();
	return data;
}

const Listings = async () => {
	const data = await getData();
	return <div>{JSON.stringify(data, null, 2)}</div>;
};

export default Listings;
