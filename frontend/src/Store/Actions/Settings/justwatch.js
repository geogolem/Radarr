import { createAction } from 'redux-actions';
import { createThunk } from 'Store/thunks';
import createSetSettingValueReducer from 'Store/Actions/Creators/Reducers/createSetSettingValueReducer';
import createFetchHandler from 'Store/Actions/Creators/createFetchHandler';
import createSaveHandler from 'Store/Actions/Creators/createSaveHandler';

//
// Variables

const section = 'settings.justwatch';

//
// Actions Types

export const FETCH_JUSTWATCH_SETTINGS = 'settings/justwatch/fetchJustwatchSettings';
export const SET_JUSTWATCH_SETTINGS_VALUE = 'settings/justwatch/setJustwatchSettingsValue';
export const SAVE_JUSTWATCH_SETTINGS = 'settings/justwatch/saveJustwatchSettings';

//
// Action Creators

export const fetchJustwatchSettings = createThunk(FETCH_JUSTWATCH_SETTINGS);
export const saveJustwatchSettings = createThunk(SAVE_JUSTWATCH_SETTINGS);
export const setJustwatchSettingsValue = createAction(SET_JUSTWATCH_SETTINGS_VALUE, (payload) => {
  return {
    section,
    ...payload
  };
});

//
// Details

export default {

  //
  // State

  defaultState: {
    isFetching: false,
    isPopulated: false,
    error: null,
    pendingChanges: {},
    isSaving: false,
    saveError: null,
    item: {}
  },

  //
  // Action Handlers

  actionHandlers: {
    [FETCH_JUSTWATCH_SETTINGS]: createFetchHandler(section, '/config/justwatch'),
    [SAVE_JUSTWATCH_SETTINGS]: createSaveHandler(section, '/config/justwatch')
  },

  //
  // Reducers

  reducers: {
    [SET_JUSTWATCH_SETTINGS_VALUE]: createSetSettingValueReducer(section)
  }

};