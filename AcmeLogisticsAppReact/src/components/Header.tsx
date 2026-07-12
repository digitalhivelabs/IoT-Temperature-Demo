export function Header() {
  return (
    <header className="bg-surface/60 backdrop-blur-xl dark:bg-surface-container/60 fixed top-0 w-full z-50 border-b border-outline-variant/20 shadow-sm flex justify-between items-center px-margin-mobile md:px-margin-desktop h-16">
      <div className="flex items-center gap-3">
        <div className="w-8 h-8 rounded-full bg-secondary-container flex items-center justify-center overflow-hidden border border-outline/30">
          <img
            className="w-full h-full object-cover"
            data-alt="A professional high-contrast portrait of a technical operator with a futuristic aesthetic, set against a dark blue background with subtle emerald green lighting accents. The style is minimalist and clean, representing high-end IoT dashboard user identity."
            src="https://lh3.googleusercontent.com/aida-public/AB6AXuB7FeonF2L9elUy_ETPSIcsjQNCTfqp9rWEAmy1q6fzh5y1wZCp3S6-Ae0yLLBdp3Eke_CWAAYBpRZzN87BGS7lU8D7thXsb5sHy8rZwXEhFBuhOPLLZiFUXDHd1bhyEwV-fetD5__AgBiQC6EvkKZx87WPFa8epQwo08FMpwPmzC6lh-fdPJNhYnM327FC1AuXKwK2n1uLWFT9mjXgx6LPP1xN7n1m4jBFDaeNsWw67CaL6NjamzvMcA"
          />
        </div>
        <span className="font-headline-md text-headline-md font-bold text-secondary dark:text-secondary-fixed">Acme Logistics IoT</span>
      </div>
      <button className="cursor-pointer active:scale-95 duration-200 text-primary dark:text-primary-fixed-dim hover:bg-surface-variant/30 p-2 rounded-xl transition-colors">
        <span className="material-symbols-outlined" data-icon="sensors">sensors</span>
      </button>
    </header>
  );
}
