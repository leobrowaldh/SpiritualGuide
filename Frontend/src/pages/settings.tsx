import settingsIcon from '../assets/settings-icon.svg';

export default function Settings(){
  function handleSettingsClick() {
    console.log("Settings clicked");
  }
  return(
    <button>
      <img src={settingsIcon} alt="settings icon" className="w-8 h-8" onClick={handleSettingsClick} />
    </button>
  )
}
