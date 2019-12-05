import PropTypes from 'prop-types';
import React, { Component } from 'react';
import { connect } from 'react-redux';
import { createSelector } from 'reselect';
import createSettingsSectionSelector from 'Store/Selectors/createSettingsSectionSelector';
import { setJustwatchSettingsValue, saveJustwatchSettings, fetchJustwatchSettings } from 'Store/Actions/settingsActions';
import { clearPendingChanges } from 'Store/Actions/baseActions';
import JustwatchSettings from './JustwatchSettings';

const SECTION = 'justwatch';

function createMapStateToProps() {
  return createSelector(
    (state) => state.settings.advancedSettings,
    createSettingsSectionSelector(SECTION),
    (advancedSettings, sectionSettings) => {
      return {
        advancedSettings,
        ...sectionSettings
      };
    }
  );
}

const mapDispatchToProps = {
  setJustwatchSettingsValue,
  saveJustwatchSettings,
  fetchJustwatchSettings,
  clearPendingChanges
};

class JustwatchSettingsConnector extends Component {

  //
  // Lifecycle

  componentDidMount() {
    this.props.fetchJustwatchSettings();
  }

  componentWillUnmount() {
    this.props.clearPendingChanges({ section: `settings.${SECTION}` });
  }

  //
  // Listeners

  onInputChange = ({ name, value }) => {
    this.props.setJustwatchSettingsValue({ name, value });
  }

  onSavePress = () => {
    this.props.saveJustwatchSettings();
  }

  //
  // Render

  render() {
    return (
      <JustwatchSettings
        onInputChange={this.onInputChange}
        onSavePress={this.onSavePress}
        {...this.props}
      />
    );
  }
}

JustwatchSettingsConnector.propTypes = {
  setJustwatchSettingsValue: PropTypes.func.isRequired,
  saveJustwatchSettings: PropTypes.func.isRequired,
  fetchJustwatchSettings: PropTypes.func.isRequired,
  clearPendingChanges: PropTypes.func.isRequired
};

export default connect(createMapStateToProps, mapDispatchToProps)(JustwatchSettingsConnector);