function App() {
    const cards = [
        {
            "style": {"backgroundColor": "var(--bg-primary)"},
            "text": "Primary",
        },
        {
            "style": {"backgroundColor": "var(--bg-surface-1)"},
            "text": "App",
        },
        {
            "style": {"backgroundColor": "var(--bg-surface-2)"},
            "text": "Card",
        },
        {
            "style": {"backgroundColor": "var(--bg-surface-3)"},
            "text": "On card",
        },
        // {
        //     "style": {"backgroundColor": "var(--bg-color-80)"},
        //     "text": "Brightest.",
        // },
    ];

    return (
        <main className="cards">
            {
                cards.map((card, index) => (
                    <section key={index} className="card" style={card["style"]}>{card["text"]}</section>
                ))
            }
        </main>
    );
}

export default App;
