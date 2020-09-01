import classNames from 'classnames';
import PropTypes from 'prop-types';
import React, { PureComponent } from 'react';
import DescriptionList from 'Components/DescriptionList/DescriptionList';
import DescriptionListItem from 'Components/DescriptionList/DescriptionListItem';
import formatBytes from 'Utilities/Number/formatBytes';
import translate from 'Utilities/String/translate';
import styles from './MovieIndexFooter.css';

class MovieIndexFooter extends PureComponent {

  render() {
    const {
      movies,
      colorImpairedMode
    } = this.props;

    const count = movies.length;
    let movieFiles = 0;
    let monitored = 0;
    let totalFileSize = 0;
    let netflix = 0;
    let primeVideo = 0;
    let tubiTV = 0;
    let hoopla = 0;

    movies.forEach((s) => {

      if (s.hasFile) {
        movieFiles += 1;
      }

      if (s.monitored) {
        monitored++;
      }

      if (s.netflixUrl) {
        netflix++;
      }
      if (s.primeVideoUrl) {
        primeVideo++;
      }
      if (s.tubiTVUrl) {
        tubiTV++;
      }
      if (s.hooplaUrl) {
        hoopla++;
      }

      totalFileSize += s.sizeOnDisk;
    });

    return (
      <div className={styles.footer}>
        <div>
          <div className={styles.legendItem}>
            <div className={styles.ended} />
            <div>
              {translate('DownloadedAndMonitored')}
            </div>
          </div>

          <div className={styles.legendItem}>
            <div className={styles.availNotMonitored} />
            <div>
              {translate('DownloadedButNotMonitored')}
            </div>
          </div>

          <div className={styles.legendItem}>
            <div className={classNames(
              styles.missingMonitored,
              colorImpairedMode && 'colorImpaired'
            )}
            />
            <div>
              {translate('MissingMonitoredAndConsideredAvailable')}
            </div>
          </div>

          <div className={styles.legendItem}>
            <div className={classNames(
              styles.missingUnmonitored,
              colorImpairedMode && 'colorImpaired'
            )}
            />
            <div>
              {translate('MissingNotMonitored')}
            </div>
          </div>

          <div className={styles.legendItem}>
            <div className={styles.queue} />
            <div>
              {translate('Queued')}
            </div>
          </div>

          <div className={styles.legendItem}>
            <div className={styles.continuing} />
            <div>
              {translate('Unreleased')}
            </div>
          </div>
        </div>

        <div className={styles.statistics}>
          <DescriptionList>
            <DescriptionListItem
              title={translate('Movies')}
              data={count}
            />

            <DescriptionListItem
              title={translate('MovieFiles')}
              data={movieFiles}
            />
          </DescriptionList>

          <DescriptionList>
            <DescriptionListItem
              title={translate('Monitored')}
              data={monitored}
            />

            <DescriptionListItem
              title={translate('Unmonitored')}
              data={count - monitored}
            />
          </DescriptionList>

          <DescriptionList>
            <DescriptionListItem
              title={translate('TotalFileSize')}
              data={formatBytes(totalFileSize)}
            />
          </DescriptionList>

          <DescriptionList>
            <DescriptionListItem
              title="Netflix"
              data={netflix}
            />

            <DescriptionListItem
              title="PrimeVideo"
              data={primeVideo}
            />

            <DescriptionListItem
              title="TubiTV"
              data={tubiTV}
            />

            <DescriptionListItem
              title="Hoopla"
              data={hoopla}
            />
          </DescriptionList>
        </div>
      </div>
    );
  }
}

MovieIndexFooter.propTypes = {
  movies: PropTypes.arrayOf(PropTypes.object).isRequired,
  colorImpairedMode: PropTypes.bool.isRequired
};

export default MovieIndexFooter;
