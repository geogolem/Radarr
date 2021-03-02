import { createSelector } from 'reselect';

function createJustwatchSettingsSelector() {
  return createSelector(
    (state) => state.settings.justwatch,
    (justwatch) => {
      return justwatch.item;
    }
  );
}

export default createJustwatchSettingsSelector;
