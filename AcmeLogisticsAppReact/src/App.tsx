import './App.css'
import { useState } from 'react';
import { Header } from './components/Header';
import { AlertCard } from './components/AlertCard';
import { TemperatureCard } from './components/TemperatureCard';
import { DeviceDetails } from './components/DeviceDetails';

function App() {
  const [selectedDevice, setSelectedDevice] = useState(false);

  const handleOpenDetails = () => setSelectedDevice(true);
  const handleCloseDetails = () => setSelectedDevice(false);

  return (
    <>
      <Header />
      {selectedDevice ? (
        <DeviceDetails
          deviceName="Thermostat Node"
          deviceId="#99283-A"
          status="ÓPTIMO"
          temperature="24.5°C"
          humidity="48.2%"
          location="Server Room"
          lastSeen="Hace 2 minutos"
          onBack={handleCloseDetails}
        />
      ) : (
        <>
          <main className="mt-20 px-margin-mobile">
            {/* Dashboard Welcome */}
            <div className="mb-8">
              <h1 className="font-display-lg text-display-lg text-on-surface">Overview</h1>
              <p className="font-body-md text-body-md text-on-surface-variant">System status: All gateways operational</p>
            </div>
            {/* Bento Grid of Categories */}
            <div className="grid grid-cols-2 gap-4 mb-10">
              <TemperatureCard value="24" unit="°C" active="12 Active" onClick={handleOpenDetails} />
            </div>

            {/* Recent Alerts Section */}
            <section className="mb-6">
              <div className="flex justify-between items-end mb-4">
                <h2 className="font-headline-sm text-headline-sm text-on-surface">Alertas Recientes</h2>
                <button className="font-label-caps text-label-caps text-secondary uppercase tracking-widest">Ver Todas</button>
              </div>
              <div className="space-y-3">
                <AlertCard
                  title="High Load Detection"
                  subtitle="NODE-042 • SERVER ROOM"
                  time="10:42 AM"
                  status="CRITICAL"
                  variant="critical"
                  icon="priority_high"
                />
                <AlertCard
                  title="Maintenance Complete"
                  subtitle="HVAC-SYS-1 • ZONE B"
                  time="09:15 AM"
                  status="RESOLVED"
                  variant="resolved"
                  icon="check_circle"
                />
              </div>
            </section>
          </main>
          {/* BottomNavBar */}
          <nav className="bg-surface/60 backdrop-blur-xl dark:bg-surface-container/60 fixed bottom-0 w-full z-50 border-t border-outline-variant/20 shadow-lg flex justify-around items-center h-20 px-4 pb-safe">
            {/* Overview Tab (Active) */}
            <div className="flex flex-col items-center justify-center bg-secondary-container/20 text-secondary dark:text-secondary-fixed rounded-xl px-4 py-1 active:scale-90 duration-200 hover:opacity-100 transition-opacity">
              <span className="material-symbols-outlined text-[24px]" data-icon="dashboard">dashboard</span>
              <span className="font-label-caps text-label-caps mt-1">Overview</span>
            </div>
            {/* Devices Tab */}
            <div className="flex flex-col items-center justify-center text-on-surface-variant dark:text-on-surface-variant opacity-70 active:scale-90 duration-200 hover:opacity-100 transition-opacity">
              <span className="material-symbols-outlined text-[24px]" data-icon="router">router</span>
              <span className="font-label-caps text-label-caps mt-1">Devices</span>
            </div>
            {/* Analytics Tab */}
            <div className="flex flex-col items-center justify-center text-on-surface-variant dark:text-on-surface-variant opacity-70 active:scale-90 duration-200 hover:opacity-100 transition-opacity">
              <span className="material-symbols-outlined text-[24px]" data-icon="analytics">analytics</span>
              <span className="font-label-caps text-label-caps mt-1">Analytics</span>
            </div>
            {/* Settings Tab */}
            <div className="flex flex-col items-center justify-center text-on-surface-variant dark:text-on-surface-variant opacity-70 active:scale-90 duration-200 hover:opacity-100 transition-opacity">
              <span className="material-symbols-outlined text-[24px]" data-icon="settings">settings</span>
              <span className="font-label-caps text-label-caps mt-1">Settings</span>
            </div>
          </nav>
        </>
      )}
    </>
  )
}

export default App
