import PropTypes from 'prop-types';
import React from 'react';
import isBefore from 'Utilities/Date/isBefore';
import { icons, kinds, sizes } from 'Helpers/Props';
import Icon from 'Components/Icon';
import ProgressBar from 'Components/ProgressBar';
import QueueDetails from 'Activity/Queue/QueueDetails';
import formatBytes from 'Utilities/Number/formatBytes';
import Label from 'Components/Label';
import styles from './JustwatchLinks.css';

function getTooltip(title, quality, size) {
  const revision = quality.revision;

  if (revision.real && revision.real > 0) {
    title += ' [REAL]';
  }

  if (revision.version && revision.version > 1) {
    title += ' [PROPER]';
  }

  if (size) {
    title += ` - ${formatBytes(size)}`;
  }

  return title;
}

function JustwatchLinks(props) {
  const {
    justwatchUrl,
    netflixUrl,
	primeVideoUrl,
	tubiTVUrl,
	hooplaUrl
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