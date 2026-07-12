type DeviceDetailsProps = {
    deviceName: string;
    deviceId: string;
    status: string;
    temperature: string;
    humidity: string;
    location: string;
    lastSeen: string;
    onBack?: () => void;
};

export function DeviceDetails({
    onBack,
}: DeviceDetailsProps) {
    return (
        <>
            {/* Top Navigation Bar */}
            <header className="fixed top-0 left-0 w-full z-50 bg-surface/60 backdrop-blur-xl dark:bg-surface-container/60 border-b border-outline-variant/20 shadow-sm flex justify-between items-center px-margin-mobile h-16">
                <div className="flex items-center gap-3">
                    <button className="p-2 -ml-2 text-on-surface-variant cursor-pointer active:scale-95 duration-200">
                        <span onClick={onBack} className="material-symbols-outlined">arrow_back</span>
                    </button>
                    <div className="flex flex-col">
                        <span className="font-headline-md text-headline-md font-bold text-secondary">Sensor TH-042</span>
                    </div>
                </div>
                <div className="flex items-center gap-4">
                    <span className="material-symbols-outlined text-primary">sensors</span>
                    <div className="w-8 h-8 rounded-full bg-surface-container-highest border border-outline-variant/30 flex items-center justify-center overflow-hidden">
                        <img className="w-full h-full object-cover" data-alt="Close-up portrait of a technical operator with a focused expression, wearing a sleek dark-colored headset, set against a dark glassmorphic background with subtle emerald green highlights. The style is professional and high-tech, reflecting the minimalist-glassmorphic aesthetic of Nexus IoT with soft bokeh lights in the background." src="https://lh3.googleusercontent.com/aida-public/AB6AXuDlUey6d-57tF5DtYwiXZGHyoj-heMb6TmdMp-Pyk-wPjFeNEx4lExwWETlgl8VaLspzXTN3C0ZRiZ-Hf7ouNJypYGYu2fneBdGeJ32jSsX96Fmt1kNmiIFaTyIlA6m1mDkCzIfU0Ai-tHQ_9P-lEidodoO6S7bkP5kwSwOPY5l1aEJWlEinXkb5MXxmdYju1oig01tMS_cwsPDHdF_zr1UJCrgYwMDoILMxrs6knVkLg9cLjScVA8KrQ" />
                    </div>
                </div>
            </header>
            <main className="pt-16 px-margin-mobile space-y-6">
                {/* Breadcrumbs & Status */}
                <section className="flex flex-col gap-2">
                    <nav className="flex items-center text-on-surface-variant/60 font-label-caps text-label-caps space-x-2">
                        <span>Devices</span>
                        <span className="material-symbols-outlined text-[14px]">chevron_right</span>
                        <span className="text-on-surface-variant">Temperature Sensors</span>
                    </nav>
                    <div className="flex items-center gap-2">
                        <div className="w-2.5 h-2.5 rounded-full bg-secondary status-pulse"></div>
                        <span className="font-label-caps text-label-caps text-secondary uppercase tracking-widest">Activo</span>
                        <span className="ml-auto text-on-surface-variant/40 font-label-caps text-label-caps">ID: NEX-TH042-XP</span>
                    </div>
                </section>
                {/* Metrics Grid (Stacked for Mobile) */}
                <section className="grid grid-cols-1 gap-4">
                    {/* Temperature Card */}
                    <div className="glass-card rounded-xl p-6 flex items-center justify-between">
                        <div>
                            <p className="font-label-caps text-label-caps text-on-surface-variant mb-1">Temperatura</p>
                            <p className="font-data-mono text-display-lg text-on-surface">24.8<span className="text-headline-sm text-primary opacity-60">°C</span></p>
                        </div>
                        <div className="flex flex-col items-end">
                            <span className="material-symbols-outlined text-secondary text-4xl" style={{fontVariationSettings: "'FILL' 1"}}>device_thermostat</span>
                            <span className="text-secondary font-label-caps text-label-caps mt-2">+1.2% ↑</span>
                        </div>
                    </div>
                </section>
                {/* Trend Chart Section */}
                <section className="glass-card rounded-xl p-6 overflow-hidden">
                    <div className="flex justify-between items-center mb-6">
                        <h3 className="font-headline-sm text-headline-sm">Tendencia (24h)</h3>
                        <div className="bg-surface-container rounded-lg p-1 flex">
                            <button className="px-3 py-1 font-label-caps text-label-caps bg-secondary/20 text-secondary rounded-md">Temp</button>
                            <button className="px-3 py-1 font-label-caps text-label-caps text-on-surface-variant/60">Hum</button>
                        </div>
                    </div>
                    <div className="h-48 w-full relative">
                        {/* Visualizing a simplified line chart with SVG */}
                        <svg className="w-full h-full overflow-visible" viewBox="0 0 400 100">
                            <defs>
                                <linearGradient id="gradient-chart" x1="0%" x2="0%" y1="0%" y2="100%">
                                    <stop offset="0%" stopColor="#4edea3" stopOpacity="0.3"></stop>
                                    <stop offset="100%" stopColor="#4edea3" stopOpacity="0"></stop>
                                </linearGradient>
                            </defs>
                            {/* Grid Lines */}
                            <line stroke="#334155" strokeDasharray="4" strokeWidth="0.5" x1="0" x2="400" y1="20" y2="20"></line>
                            <line stroke="#334155" strokeDasharray="4" strokeWidth="0.5" x1="0" x2="400" y1="50" y2="50"></line>
                            <line stroke="#334155" strokeDasharray="4" strokeWidth="0.5" x1="0" x2="400" y1="80" y2="80"></line>
                            {/* Path Area */}
                            <path d="M0 80 Q 50 20, 100 50 T 200 40 T 300 60 T 400 30 L 400 100 L 0 100 Z" fill="url(#gradient-chart)"></path>
                            {/* Path Line */}   
                            <path d="M0 80 Q 50 20, 100 50 T 200 40 T 300 60 T 400 30" fill="none" stroke="#4edea3" strokeLinecap="round" strokeWidth="3"></path>
                            {/* Current Point */}
                            <circle cx="400" cy="30" fill="#4edea3" r="4"></circle>
                            <circle cx="400" cy="30" fill="#4edea3" opacity="0.2" r="8"></circle>
                        </svg>
                        <div className="flex justify-between mt-4 font-label-caps text-label-caps text-on-surface-variant/40">
                            <span>00:00</span>
                            <span>06:00</span>
                            <span>12:00</span>
                            <span>18:00</span>
                            <span>Ahora</span>
                        </div>
                    </div>
                </section>
                {/* Telemetry History Section */}
                <section className="glass-card rounded-xl overflow-hidden mb-8">
                    <div className="px-6 py-4 flex justify-between items-center border-b border-outline-variant/20">
                        <h3 className="font-headline-sm text-headline-sm">Historial de Telemetría</h3>
                        <button className="text-secondary font-label-caps text-label-caps flex items-center">
                            Ver Todo <span className="material-symbols-outlined ml-1 text-sm">open_in_new</span>
                        </button>
                    </div>
                    <div className="overflow-x-auto hide-scrollbar">
                        <table className="w-full text-left min-w-[400px]">
                            <thead className="bg-surface-container-high/40 font-label-caps text-label-caps text-on-surface-variant">
                                <tr>
                                    <th className="px-6 py-3 font-semibold">Timestamp</th>
                                    <th className="px-6 py-3 font-semibold text-center">Seq ID</th>
                                    <th className="px-6 py-3 font-semibold text-right">Temp</th>
                                    <th className="px-6 py-3 font-semibold text-right">Hum</th>
                                </tr>
                            </thead>
                            <tbody className="divide-y divide-outline-variant/10 font-body-md text-body-md">
                                <tr className="hover:bg-surface-variant/20 transition-colors">
                                    <td className="px-6 py-4 text-on-surface">14:32:05</td>
                                    <td className="px-6 py-4 text-center font-data-mono text-xs opacity-60">#8942</td>
                                    <td className="px-6 py-4 text-right text-secondary font-medium">24.8°C</td>
                                    <td className="px-6 py-4 text-right text-on-surface">45%</td>
                                </tr>
                                <tr className="hover:bg-surface-variant/20 transition-colors">
                                    <td className="px-6 py-4 text-on-surface">14:31:05</td>
                                    <td className="px-6 py-4 text-center font-data-mono text-xs opacity-60">#8941</td>
                                    <td className="px-6 py-4 text-right text-secondary font-medium">24.7°C</td>
                                    <td className="px-6 py-4 text-right text-on-surface">45%</td>
                                </tr>
                                <tr className="hover:bg-surface-variant/20 transition-colors">
                                    <td className="px-6 py-4 text-on-surface">14:30:05</td>
                                    <td className="px-6 py-4 text-center font-data-mono text-xs opacity-60">#8940</td>
                                    <td className="px-6 py-4 text-right text-secondary font-medium">24.8°C</td>
                                    <td className="px-6 py-4 text-right text-on-surface">46%</td>
                                </tr>
                                <tr className="hover:bg-surface-variant/20 transition-colors">
                                    <td className="px-6 py-4 text-on-surface">14:29:05</td>
                                    <td className="px-6 py-4 text-center font-data-mono text-xs opacity-60">#8939</td>
                                    <td className="px-6 py-4 text-right text-secondary font-medium">24.9°C</td>
                                    <td className="px-6 py-4 text-right text-on-surface">46%</td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                    <div className="px-6 py-3 bg-surface-container/20 text-center">
                        <p className="font-label-caps text-label-caps text-on-surface-variant/40">Desliza para ver más</p>
                    </div>
                </section>
            </main>
            {/* Bottom Navigation Bar (Mandatory Shell) */}
            <nav className="fixed bottom-0 w-full z-50 bg-surface/60 backdrop-blur-xl dark:bg-surface-container/60 border-t border-outline-variant/20 shadow-lg flex justify-around items-center h-20 px-4 pb-safe md:hidden">
                <a className="flex flex-col items-center justify-center text-on-surface-variant dark:text-on-surface-variant opacity-70 hover:opacity-100 transition-opacity active:scale-90 duration-200" href="#">
                    <span className="material-symbols-outlined">dashboard</span>
                    <span className="font-label-caps text-label-caps mt-1">Overview</span>
                </a>
                <a className="flex flex-col items-center justify-center bg-secondary-container/20 text-secondary dark:text-secondary-fixed rounded-xl px-4 py-1 active:scale-90 duration-200" href="#">
                    <span className="material-symbols-outlined" style={{ fontVariationSettings: "'FILL' 1" }}>router</span>
                    <span className="font-label-caps text-label-caps mt-1">Devices</span>
                </a>
                <a className="flex flex-col items-center justify-center text-on-surface-variant dark:text-on-surface-variant opacity-70 hover:opacity-100 transition-opacity active:scale-90 duration-200" href="#">
                    <span className="material-symbols-outlined">analytics</span>
                    <span className="font-label-caps text-label-caps mt-1">Analytics</span>
                </a>
                <a className="flex flex-col items-center justify-center text-on-surface-variant dark:text-on-surface-variant opacity-70 hover:opacity-100 transition-opacity active:scale-90 duration-200" href="#">
                    <span className="material-symbols-outlined">settings</span>
                    <span className="font-label-caps text-label-caps mt-1">Settings</span>
                </a>
            </nav>
            {/* Background Atmospheric Effect */}
            <div className="fixed inset-0 -z-10 pointer-events-none opacity-20">

            </div>
            {/* END MAIN CONTENT */}

        </>
    );
}
