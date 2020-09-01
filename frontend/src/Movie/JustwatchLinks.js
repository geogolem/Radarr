import PropTypes from 'prop-types';
import React from 'react';
import Icon from 'Components/Icon';
import { icons } from 'Helpers/Props';
import styles from './JustwatchLinks.css';

function JustwatchLinks(props) {
  const {
    justwatchUrl
  } = props;

  if (justwatchUrl) {
    return (
      <div className={styles.center}>
        <Icon
          name={icons.DOWNLOADING}
          title={` ${justwatchUrl}`}
        />
      </div>
    );
  }

}

JustwatchLinks.propTypes = {
  justwatchUrl: PropTypes.string,
  netflixUrl: PropTypes.string,
  primeVideoUrl: PropTypes.string,
  tubiTVUrl: PropTypes.string,
  hooplaUrl: PropTypes.string
};

export default JustwatchLinks;
