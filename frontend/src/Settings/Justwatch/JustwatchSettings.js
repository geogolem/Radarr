import PropTypes from 'prop-types';
import React, { Component } from 'react';
import FieldSet from 'Components/FieldSet';
import Form from 'Components/Form/Form';
import FormGroup from 'Components/Form/FormGroup';
import FormInputGroup from 'Components/Form/FormInputGroup';
import FormLabel from 'Components/Form/FormLabel';
import LoadingIndicator from 'Components/Loading/LoadingIndicator';
import PageContent from 'Components/Page/PageContent';
import PageContentBody from 'Components/Page/PageContentBody';
import { inputTypes } from 'Helpers/Props';
import SettingsToolbarConnector from 'Settings/SettingsToolbarConnector';

export const justwatchLocaleOptions = [
  { key: 'en_CA', value: 'Canada' },
  { key: 'en_US', value: 'United States' },
  { key: 'en_GB', value: 'Great Britain' },
  { key: 'en_AU', value: 'Austrailia' },
  { key: 'en_IE', value: 'Ireland' },
  { key: 'en_MX', value: 'Mexico' },
  { key: 'fr_FR', value: 'France' },
  { key: 'pt_BR', value: 'Brazil' },
  { key: 'de_DE', value: 'Germany' },
  { key: 'es_ES', value: 'Spain' },
  { key: 'en_NL', value: 'Netherlands' },
  { key: 'en_ZA', value: 'South Africa' },
  { key: 'en_NZ', value: 'New Zealand' }
];

export const supportLevelOptions = [
  { key: 'enabled', value: 'Enabled' },
  { key: 'disabled', value: 'Disabled-Clear existing data' },
  { key: 'disabledKeep', value: 'Disabled-Keep existing data' }
];

export const ignoreOptions = [
  { key: 'disabled', value: 'Disabled' },
  { key: 'enabled', value: 'Enabled' }
];

class JustwatchSettings extends Component {

  //
  // Render

  render() {
    const {
      isFetching,
      error,
      settings,
      hasSettings,
      onInputChange,
      onSavePress,
      ...otherProps
    } = this.props;

    return (
      <PageContent title="Justwatch Settings">
        <SettingsToolbarConnector
          {...otherProps}
          onSavePress={onSavePress}
        />

        <PageContentBody>
          {
            isFetching &&
              <LoadingIndicator />
          }

          {
            !isFetching && error &&
              <div>Unable to load Justwatch settings</div>
          }

          {
            hasSettings && !error && !isFetching &&
              <Form
                id="justwatchSettings"
                {...otherProps}
              >
                <FieldSet legend="General">
                  <FormGroup>
                    <FormLabel>Locale</FormLabel>

                    <FormInputGroup
                      type={inputTypes.SELECT}
                      name="justwatchLocale"
                      values={justwatchLocaleOptions}
                      onChange={onInputChange}
                      {...settings.justwatchLocale}
                    />
                  </FormGroup>
                </FieldSet>
                <FieldSet legend="Netflix Options">
                  <FormGroup>
                    <FormLabel>Support Level</FormLabel>
                    <FormInputGroup
                      type={inputTypes.SELECT}
                      name="enableNetflix"
                      values={supportLevelOptions}
                      onChange={onInputChange}
                      helpText="If/how Netflix data is obtained and set (on refresh)"
                      {...settings.enableNetflix}
                    />
                  </FormGroup>
                  <FormGroup>
                    <FormLabel>Ignore Titles</FormLabel>
                    <FormInputGroup
                      type={inputTypes.CHECK}
                      name="ignoreNetflixTitles"
                      helpText="Unmonitor titles if they are on Netflix (on refresh)"
                      onChange={onInputChange}
                      {...settings.ignoreNetflixTitles}
                    />
                  </FormGroup>
                </FieldSet>
                <FieldSet legend="Amazon Prime Video Options">
                  <FormGroup>
                    <FormLabel>Support Level</FormLabel>
                    <FormInputGroup
                      type={inputTypes.SELECT}
                      name="enablePrimeVideo"
                      values={supportLevelOptions}
                      onChange={onInputChange}
                      helpText="If/how Amazon Prime Video data is obtained and set (on refresh)"
                      {...settings.enablePrimeVideo}
                    />
                  </FormGroup>
                  <FormGroup>
                    <FormLabel>Ignore Titles</FormLabel>
                    <FormInputGroup
                      type={inputTypes.CHECK}
                      name="ignorePrimeVideoTitles"
                      helpText="Unmonitor titles if they are on Amazon Prime Video(on refresh)"
                      onChange={onInputChange}
                      {...settings.ignorePrimeVideoTitles}
                    />
                  </FormGroup>

                </FieldSet>
                <FieldSet legend="TubiTV Options">
                  <FormGroup>
                    <FormLabel>Support Level</FormLabel>
                    <FormInputGroup
                      type={inputTypes.SELECT}
                      name="enableTubiTV"
                      values={supportLevelOptions}
                      onChange={onInputChange}
                      helpText="If/how TubiTV data is obtained and set (on refresh)"
                      {...settings.enableTubiTV}
                    />
                  </FormGroup>
                  <FormGroup>
                    <FormLabel>Ignore Titles</FormLabel>
                    <FormInputGroup
                      type={inputTypes.CHECK}
                      name="ignoreTubiTVTitles"
                      helpText="Unmonitor titles if they are on TubiTV(on refresh)"
                      onChange={onInputChange}
                      {...settings.ignoreTubiTVTitles}
                    />
                  </FormGroup>

                </FieldSet>
                <FieldSet legend="Hoopla Options">
                  <FormGroup>
                    <FormLabel>Support Level</FormLabel>
                    <FormInputGroup
                      type={inputTypes.SELECT}
                      name="enableHoopla"
                      values={supportLevelOptions}
                      onChange={onInputChange}
                      helpText="If/how Hoopla data is obtained and set (on refresh)"
                      {...settings.enableHoopla}
                    />
                  </FormGroup>
                  <FormGroup>
                    <FormLabel>Ignore Titles</FormLabel>
                    <FormInputGroup
                      type={inputTypes.CHECK}
                      name="ignoreHooplaTitles"
                      helpText="Unmonitor titles if they are on Hoopla(on refresh)"
                      onChange={onInputChange}
                      {...settings.ignoreHooplaTitles}
                    />
                  </FormGroup>
                </FieldSet>

              </Form>
          }
        </PageContentBody>
      </PageContent>
    );
  }

}

JustwatchSettings.propTypes = {
  isFetching: PropTypes.bool.isRequired,
  error: PropTypes.object,
  settings: PropTypes.object.isRequired,
  hasSettings: PropTypes.bool.isRequired,
  onSavePress: PropTypes.func.isRequired,
  onInputChange: PropTypes.func.isRequired
};

export default JustwatchSettings;
